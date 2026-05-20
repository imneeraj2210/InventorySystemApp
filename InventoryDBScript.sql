--
-- PostgreSQL database dump
--

-- Dumped from database version 15.4
-- Dumped by pg_dump version 15.4

-- Started on 2026-05-20 17:09:42

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 221 (class 1259 OID 24918)
-- Name: cart; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cart (
    id integer NOT NULL,
    user_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer DEFAULT 1 NOT NULL
);


ALTER TABLE public.cart OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 24917)
-- Name: cart_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cart_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.cart_id_seq OWNER TO postgres;

--
-- TOC entry 3375 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_id_seq OWNED BY public.cart.id;


--
-- TOC entry 219 (class 1259 OID 16615)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    id integer NOT NULL,
    user_id integer,
    product_id integer,
    quantity integer NOT NULL,
    order_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    status character varying(20) DEFAULT 'Pending'::character varying
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16614)
-- Name: orders_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orders_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orders_id_seq OWNER TO postgres;

--
-- TOC entry 3376 (class 0 OID 0)
-- Dependencies: 218
-- Name: orders_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_id_seq OWNED BY public.orders.id;


--
-- TOC entry 217 (class 1259 OID 16608)
-- Name: products; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.products (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    stock_quantity integer NOT NULL,
    image_url text
);


ALTER TABLE public.products OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 16607)
-- Name: products_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.products_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.products_id_seq OWNER TO postgres;

--
-- TOC entry 3377 (class 0 OID 0)
-- Dependencies: 216
-- Name: products_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_id_seq OWNED BY public.products.id;


--
-- TOC entry 215 (class 1259 OID 16598)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    role character varying(20) DEFAULT 'Customer'::character varying,
    email character varying(100)
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 16597)
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_id_seq OWNER TO postgres;

--
-- TOC entry 3378 (class 0 OID 0)
-- Dependencies: 214
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- TOC entry 223 (class 1259 OID 24936)
-- Name: website_settings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.website_settings (
    id integer NOT NULL,
    site_name character varying(200),
    logo_url text
);


ALTER TABLE public.website_settings OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 24935)
-- Name: website_settings_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.website_settings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.website_settings_id_seq OWNER TO postgres;

--
-- TOC entry 3379 (class 0 OID 0)
-- Dependencies: 222
-- Name: website_settings_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.website_settings_id_seq OWNED BY public.website_settings.id;


--
-- TOC entry 3199 (class 2604 OID 24921)
-- Name: cart id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart ALTER COLUMN id SET DEFAULT nextval('public.cart_id_seq'::regclass);


--
-- TOC entry 3196 (class 2604 OID 16618)
-- Name: orders id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN id SET DEFAULT nextval('public.orders_id_seq'::regclass);


--
-- TOC entry 3195 (class 2604 OID 16611)
-- Name: products id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN id SET DEFAULT nextval('public.products_id_seq'::regclass);


--
-- TOC entry 3193 (class 2604 OID 16601)
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- TOC entry 3201 (class 2604 OID 24939)
-- Name: website_settings id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.website_settings ALTER COLUMN id SET DEFAULT nextval('public.website_settings_id_seq'::regclass);


--
-- TOC entry 3367 (class 0 OID 24918)
-- Dependencies: 221
-- Data for Name: cart; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart (id, user_id, product_id, quantity) FROM stdin;
\.


--
-- TOC entry 3365 (class 0 OID 16615)
-- Dependencies: 219
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (id, user_id, product_id, quantity, order_date, status) FROM stdin;
15	4	8	5	2026-05-19 22:46:05.581896	Pending
16	4	9	4	2026-05-19 22:46:05.581896	Pending
18	4	8	2	2026-05-20 14:08:26.886037	Rejected
17	4	7	1	2026-05-20 14:08:26.886037	Approved
\.


--
-- TOC entry 3363 (class 0 OID 16608)
-- Dependencies: 217
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (id, name, price, stock_quantity, image_url) FROM stdin;
9	Cooler	100.00	6	/images/c4f7cd14-b32a-4cef-bb2b-fe033efe7f4b.jpg
10	iPhone	100.00	10	/images/daa25187-fa0e-4d60-b48f-bbaffcd7213b.jpg
11	LED	50.00	10	/images/dddad9e0-4ea8-45a3-b142-9aec1965c0eb.jpg
12	Washing Machine	30.00	30	/images/343eed7f-094b-49b4-9a59-f968cbbf74fa.jpg
13	Air Conditioner	40.00	10	/images/0a3f7f57-6219-4835-ab49-1796d5df5c45.jpg
14	Fan	25.00	30	/images/a69a3c87-5e5b-4a95-bdc6-5eea65d46968.jpg
15	Heater	30.00	5	/images/792f07f7-4c20-43f7-b926-13d0afebe03f.jpg
7	Laptop	120.00	19	/images/53dd0322-026e-42d9-b6a8-de357d6acc39.jpg
8	Smart Watch	10.00	93	/images/8f242d93-f4f9-4b57-bea0-d33ff133b5d9.jpg
\.


--
-- TOC entry 3361 (class 0 OID 16598)
-- Dependencies: 215
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, username, password, role, email) FROM stdin;
1	admin	admin123	Admin	\N
2	user	user123	Customer	\N
3	test	test123	Customer	\N
4	imneeraj	Welcome@12345	Customer	imneeraj@test.com
5	imneeraj2210	Welcome@123	Customer	sample@demo.com
\.


--
-- TOC entry 3369 (class 0 OID 24936)
-- Dependencies: 223
-- Data for Name: website_settings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.website_settings (id, site_name, logo_url) FROM stdin;
1	SampleInventory	/image/3c148f7c-6d1d-4768-a2da-5ca2b41f1d55.png
\.


--
-- TOC entry 3380 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_id_seq', 5, true);


--
-- TOC entry 3381 (class 0 OID 0)
-- Dependencies: 218
-- Name: orders_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_id_seq', 18, true);


--
-- TOC entry 3382 (class 0 OID 0)
-- Dependencies: 216
-- Name: products_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_id_seq', 15, true);


--
-- TOC entry 3383 (class 0 OID 0)
-- Dependencies: 214
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_id_seq', 5, true);


--
-- TOC entry 3384 (class 0 OID 0)
-- Dependencies: 222
-- Name: website_settings_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.website_settings_id_seq', 1, true);


--
-- TOC entry 3211 (class 2606 OID 24924)
-- Name: cart cart_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart
    ADD CONSTRAINT cart_pkey PRIMARY KEY (id);


--
-- TOC entry 3209 (class 2606 OID 16621)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (id);


--
-- TOC entry 3207 (class 2606 OID 16613)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (id);


--
-- TOC entry 3203 (class 2606 OID 16604)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3205 (class 2606 OID 16606)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 3213 (class 2606 OID 24943)
-- Name: website_settings website_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.website_settings
    ADD CONSTRAINT website_settings_pkey PRIMARY KEY (id);


--
-- TOC entry 3216 (class 2606 OID 24930)
-- Name: cart fk_cart_product; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart
    ADD CONSTRAINT fk_cart_product FOREIGN KEY (product_id) REFERENCES public.products(id);


--
-- TOC entry 3217 (class 2606 OID 24925)
-- Name: cart fk_cart_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart
    ADD CONSTRAINT fk_cart_user FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- TOC entry 3214 (class 2606 OID 16627)
-- Name: orders orders_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.products(id);


--
-- TOC entry 3215 (class 2606 OID 16622)
-- Name: orders orders_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id);


-- Completed on 2026-05-20 17:09:43

--
-- PostgreSQL database dump complete
--

